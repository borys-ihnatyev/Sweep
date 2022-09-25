import type { TrackInfo } from "./types";

const baseUrl = "http://localhost:5018";
const jsonContentType = "application/json";

type ParseRequest = {
  value: string;
};

type ParseResponse =
  | {
      success: false;
      error: string;
    }
  | {
      success: true;
      data: TrackInfo;
    };

export async function parseTrackInfo(name: string): Promise<TrackInfo> {
  const body: ParseRequest = {
    value: name,
  };

  return fetch(`${baseUrl}/parse`, {
    method: "POST",
    body: JSON.stringify(body),
    headers: {
      "Content-Type": jsonContentType,
      Accept: jsonContentType,
    },
  })
    .then((response) => response.json())
    .then((response) => {
      const parseResponse = response as ParseResponse;
      if (parseResponse.success) {
        return parseResponse.data;
      }

      return Promise.reject(parseResponse.error);
    });
}

type RenameResponse = {};

type RenameRequest = {
  path: string;
  newName: string;
};

export async function renameTrack(
  request: RenameRequest
): Promise<ParseResponse> {
  return fetch(`${baseUrl}/rename`, {
    method: "POST",
    body: JSON.stringify(request),
    headers: {
      "Content-Type": jsonContentType,
      Accept: jsonContentType,
    },
  })
    .then((response) => response.json())
    .then((response) => response as ParseResponse);
}
