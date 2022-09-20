import type { TrackInfo } from "./TrackInfo";

const baseUrl = "http://localhost:5018";
const jsonContentType = "application/json";

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
  return fetch(`${baseUrl}/parse`, {
    method: "POST",
    body: JSON.stringify({
      value: name,
    }),
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
