import Chokidar from "chokidar";
import Path from "node:path";
import OS from "node:os";
import { parseTrackInfo } from "./parsingService";
import { MessagingService } from "./messagingService";

const watchPath = Path.resolve(OS.homedir(), "Downloads");
const watchPattern = `${watchPath}/*.(mp3|alac|flac|wav)`;

const messagingService = MessagingService();

Chokidar.watch(watchPattern, { ignoreInitial: true, depth: 0 }).on(
  "add",
  async (filePath) => {
    const { name, ext, dir } = Path.parse(filePath);
    try {
      console.time("request");
      const trackInfo = await parseTrackInfo(name);
      console.log(trackInfo);
      messagingService.sendMessage(trackInfo);
    } catch (error) {
      console.error("Parse error", error);
    } finally {
      console.timeEnd("request");
    }
  }
);
