import EventEmitter from "node:events";
import { Server as HTTPServer } from "node:http";
import { Server as SocketServer, Socket } from "socket.io";
import { ConfirmEditRequest, ConfirmEditResponse } from "./Payload";

type Action<T> = (payload: T) => void;

type ListenEvents = {
  confirmEdit: Action<ConfirmEditResponse>;
};

type EmitEvents = {
  confirmEdit: Action<ConfirmEditRequest>;
};

export class MessagingService {
  private readonly eventEmitter = new EventEmitter();
  private readonly httpServer = new HTTPServer();
  private readonly socketServer = new SocketServer<ListenEvents, EmitEvents>(
    this.httpServer
  );

  constructor(port = 3737, host = "localhost") {
    this.socketServer.on("connection", (socket) => {
      console.log("Socked connected");

      socket
        .on("confirmEdit", (payload) => {
          this.eventEmitter.emit("confirmEdit", payload);
        })
        .once("disconnect", () => console.log("Socked disconnected"));
    });

    this.httpServer.listen(port, () => {
      console.log(`Messaging service listening http://${host}:${port}`);
    });
  }

  sendMessage<Name extends keyof EmitEvents>(
    name: Name,
    ...params: Parameters<EmitEvents[Name]>
  ) {
    this.socketServer.emit(name, ...params);
  }

  addListener<Event extends keyof ListenEvents>(
    name: Event,
    callback: ListenEvents[Event]
  ): VoidFunction {
    this.eventEmitter.on(name, callback);

    return () => {
      this.eventEmitter.off(name, callback);
    };
  }
}
