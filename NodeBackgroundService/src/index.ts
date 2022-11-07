import Path from "node:path";
import OS from "node:os";
import { MessagingService } from "./MessagingService";
import { WatcherService } from "./WatcherService";
import { TrackInfoService } from "./TrackInfoService";

const watchPath = Path.resolve(OS.homedir(), "Downloads");
const watcherService = new WatcherService(watchPath);
const messagingService = new MessagingService();
const trackInfoService = new TrackInfoService();

// ToDo use conf
let shouldUseNotification = true;

watcherService.addListener(async (location) => {
  try {
    console.log("File renamed", location.parsedPath.name);

    const request = await trackInfoService.tryCreateEditRequest(location);
    if (!request) {
      console.log("Skip rename");
      return;
    }

    if (shouldUseNotification) {
      messagingService.sendMessage("confirmEdit", request);
    } else {
      await trackInfoService.performEditRequest(request.id);
    }
  } catch (error) {
    console.error("Edit request error:", error);
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

messagingService.addListener("useNotificationsToggle", ({ enabled }) => {
  shouldUseNotification = enabled;
});

watcherService.start();
