import EventEmitter from "node:events";
import { Server as HTTPServer } from "node:http";
import { Server as SocketServer } from "socket.io";
import { ListenEvents, EmitEvents } from "./types";

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
        .onAny((name, payload) => {
          this.eventEmitter.emit(name, payload);
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
