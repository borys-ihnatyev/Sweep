import { TrackInfo } from "../TrackInfo";

export type ConfirmEditRequest = {
  id: string;
  originalName: string;
  trackInfo: TrackInfo;
};

export type ConfirmEditResponse = {
  id: string;
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
