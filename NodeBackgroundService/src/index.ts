import Path from "node:path";
import OS from "node:os";
import { MessagingService } from "./MessagingService";
import { WatcherService } from "./WatcherService";
import { TrackInfoService } from "./TrackInfoService";

const watchPath = Path.resolve(OS.homedir(), "Downloads");
const watcherService = new WatcherService(watchPath);
const messagingService = new MessagingService();
const trackInfoService = new TrackInfoService();

watcherService.addListener(async (location) => {
  try {
    const request = await trackInfoService.tryCreateEditRequest(location);
    if (!request) {
      return;
    }

    messagingService.sendMessage("confirmEdit", request);
  } catch (error) {
    console.error("Edit request creation error:", error);
  }
});

messagingService.addListener("confirmEdit", (response) => {
  const { requestId, resolution } = response;

  if (resolution.type === "deny") {
    trackInfoService.cancelEditRequest(requestId);
    return;
  }

  if (resolution.type === "allow") {
    trackInfoService.performEditRequest(requestId);
    return;
  }

  if (resolution.type === "rename") {
    trackInfoService.performEditRequest(requestId, resolution.newName);
    return;
  }
});

messagingService.addListener("watchToggle", ({ enabled }) => {
  if (enabled) {
    watcherService.start();
  } else {
    watcherService.stop();
  }
});

watcherService.start();
