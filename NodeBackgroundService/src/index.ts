import Path from "node:path";
import FS from "node:fs/promises";
import OS from "node:os";
import { randomUUID } from "node:crypto";
import { Promise as ID3 } from "node-id3";
import { parseTrackInfo } from "./LegacyService";
import { ConfirmEditRequest, MessagingService } from "./MessagingService";
import { WatcherService } from "./WatcherService";

const watchPath = Path.resolve(OS.homedir(), "Downloads");
const watcherService = new WatcherService(watchPath);
const messagingService = new MessagingService();

const pendingRequestsStore = new Map<string, ConfirmEditRequest>();

watcherService.addListener(async (location) => {
  try {
    const {
      parsedPath: { name },
    } = location;

    const request: ConfirmEditRequest = {
      id: randomUUID(),
      location,
      trackInfo: await parseTrackInfo(name),
    };

    pendingRequestsStore.set(request.id, request);

    messagingService.sendMessage("confirmEdit", request);
  } catch (error) {
    console.error("Parse error", error);
  }
});

messagingService.addListener("confirmEdit", async (payload) => {
  const { requestId, resolution } = payload;
  const request = pendingRequestsStore.get(payload.requestId);
  if (!request) {
    return;
  }

  if (resolution.type === "deny") {
    pendingRequestsStore.delete(requestId);
    return;
  }

  if (resolution.type === "allow") {
    const {
      location: { path, parsedPath },
      trackInfo,
    } = request;
    try {
      await ID3.update(
        {
          artist: trackInfo.artistsString,
          title: trackInfo.fullTitle,
          remixArtist: trackInfo.remixArtistsString,
        },
        path
      );
    } catch (error) {
      console.log("Update tags error", error);
    }

    const newPath = Path.resolve(
      parsedPath.dir,
      trackInfo.fullName + parsedPath.ext
    );

    try {
      await FS.rename(path, newPath);
    } catch (error) {
      console.log("Rename error", error);
    }

    return;
  }

  if (resolution.type === "rename") {
    // ToDo
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
