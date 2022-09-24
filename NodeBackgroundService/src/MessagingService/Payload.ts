import { TrackInfo } from "../TrackInfo";

export type ConfirmEditRequest = {
  originalName: string;
  trackInfo: TrackInfo;
};

export type ConfirmEditResponse =
  | {
      resolution: "allow";
    }
  | {
      resolution: "deny";
    }
  | {
      resolution: "rename";
      newName: string;
    };
