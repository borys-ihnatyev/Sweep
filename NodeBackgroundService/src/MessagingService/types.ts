import { TrackInfo } from "../LegacyService/types";
import { FileLocation } from "../WatcherService";

type Action<T> = (payload: T) => void;

export type ListenEvents = {
  confirmEdit: Action<ConfirmEditResponse>;
  watchToggle: Action<ToggleRequest>;
  useNotificationsToggle: Action<ToggleRequest>;
};

export type EmitEvents = {
  confirmEdit: Action<ConfirmEditRequest>;
};

export type ConfirmEditRequest = {
  id: string;
  location: FileLocation;
  trackInfo: TrackInfo;
};

export type ConfirmEditResponse = {
  requestId: string;
  resolution: ConfirmEditResponseResolution;
};

export type ConfirmEditResponseResolution =
  | {
      type: "allow";
    }
  | {
      type: "deny";
    }
  | {
      type: "rename";
      newName: string;
    };

type ToggleRequest = {
  enabled: boolean;
};
