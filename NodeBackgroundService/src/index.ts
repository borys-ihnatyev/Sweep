import Chokidar from "chokidar";
import Path from "node:path";
import OS from "node:os";
import { randomUUID } from "node:crypto";
import { parseTrackInfo } from "./parsingService";
import { MessagingService } from "./MessagingService";

const watchPath = Path.resolve(OS.homedir(), "Downloads");

const allowedExtensions = ["mp3", "m4a", "wav"];
const watchPattern = `${watchPath}/*.(${allowedExtensions.join("|")})`;

const messagingService = new MessagingService();
messagingService.addListener("confirmEdit", (payload) => {
  console.log("confirmEdit response:", payload);
});

Chokidar.watch(watchPattern, { ignoreInitial: true, depth: 0 }).on(
  "add",
  async (filePath) => {
    const { name, ext, dir } = Path.parse(filePath);
    try {
      messagingService.sendMessage("confirmEdit", {
        id: randomUUID(),
        originalName: name,
        trackInfo: await parseTrackInfo(name),
      });
    } catch (error) {
      console.error("Parse error", error);
    }
  }
);
