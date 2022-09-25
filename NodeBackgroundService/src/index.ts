import Path from "node:path";
import OS from "node:os";
import { randomUUID } from "node:crypto";
import { parseTrackInfo } from "./ParsingService";
import { ConfirmEditRequest, MessagingService } from "./MessagingService";
import { WatcherService } from "./WatcherService";

const watchPath = Path.resolve(OS.homedir(), "Downloads");
const watcherService = new WatcherService(watchPath);
const messagingService = new MessagingService();

watcherService.addListener(async ({ name }) => {
  try {
    const request: ConfirmEditRequest = {
      id: randomUUID(),
      originalName: name,
      trackInfo: await parseTrackInfo(name),
    };

    messagingService.sendMessage("confirmEdit", request);
  } catch (error) {
    console.error("Parse error", error);
  }
});

messagingService.addListener("confirmEdit", (payload) => {
  // Todo:
  console.log("confirmEdit response:", payload);
});

messagingService.addListener("watchToggle", ({ enabled }) => {
  if (enabled) {
    watcherService.start();
  } else {
    watcherService.stop();
  }
});
