import Path from "node:path";
import FS from "node:fs/promises";
import { randomUUID } from "node:crypto";
import { Promise as ID3 } from "node-id3";
import { parseTrackInfo } from "./LegacyService";
import { ConfirmEditRequest } from "./MessagingService";
import { FileLocation } from "./WatcherService";

const SWEEPED_TAG = "#sweeped";

export class TrackInfoService {
  private readonly pendingRequests = new Map<string, ConfirmEditRequest>();

  async tryCreateEditRequest(
    fileLocation: FileLocation
  ): Promise<ConfirmEditRequest | undefined> {
    /*     const shouldCreate = await this.isEdited(fileLocation);
    if (!shouldCreate) {
      return;
    } */

    const {
      parsedPath: { name },
    } = fileLocation;

    const request: ConfirmEditRequest = {
      id: randomUUID(),
      location: fileLocation,
      trackInfo: await parseTrackInfo(name),
    };

    this.pendingRequests.set(request.id, request);

    return request;
  }

  private async isEdited(location: FileLocation): Promise<boolean> {
    const tags = await ID3.read(location.path);
    return !tags.comment?.text.includes(SWEEPED_TAG) ?? true;
  }

  cancelEditRequest(requestId: string): void {
    this.pendingRequests.delete(requestId);
  }

  async performEditRequest(requestId: string, newName?: string): Promise<void> {
    const request = this.pendingRequests.get(requestId);
    if (!request) {
      return;
    }

    const {
      location: { path, parsedPath },
    } = request;

    const trackInfo = newName
      ? await parseTrackInfo(newName)
      : request.trackInfo;

    const tags = await ID3.read(path).catch(() => undefined);

    try {
      await ID3.update(
        {
          artist: trackInfo.artistsString,
          title: trackInfo.fullTitle,
          remixArtist: trackInfo.remixArtistsString,
          comment: {
            language: tags?.comment?.language ?? "en",
            text: `${SWEEPED_TAG} ${tags?.comment?.text ?? ""}`.trim(),
          },
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

    this.pendingRequests.delete(requestId);
  }
}
