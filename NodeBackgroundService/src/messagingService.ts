import { Server, Socket } from "node:net";

const port = 3737;
const hostname = "localhost";

export type MessagingService = {
  sendMessage<T extends {}>(message: T, cb?: (error?: Error) => void): void;
  stop(): void;
};

export function MessagingService(): MessagingService {
  const sockets = new Set<Socket>();
  const server = new Server();

  server
    .on("connection", (socket) => {
      sockets.add(socket);
      socket
        .once("close", (hadError) => {
          sockets.delete(socket);
          console.log(
            `Socket closed ${socket.remoteAddress}${
              hadError ? " with error" : ""
            }`
          );
        })
        .on("data", (buffer) => {
          console.log("Message: " + buffer.toString());
        });
    })
    .on("error", (error) => {
      console.log("Socket error", error);
    })
    .listen(port, hostname, () => {
      console.log(`Messaging service listening tcp://${hostname}:${port}`);
    });

  return {
    sendMessage(message, cb?: (error?: Error) => void) {
      const json = JSON.stringify(message);
      sockets.forEach((socket) => {
        socket.write(json, cb);
      });
    },
    stop() {
      server.close();
    },
  };
}
