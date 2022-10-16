mod rest_track_info_parser;

use async_trait::async_trait;
use serde::{Deserialize, Serialize};

use self::rest_track_info_parser::RestTrackInfoParser;

#[derive(Clone, Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct TrackInfo {
    pub full_name: String,
    pub full_title: String,
    pub title: String,

    pub artists_string: String,
    pub remix_artists_string: String,

    pub is_remix: bool,
    pub mix_type: String,
}


#[async_trait]
pub trait TrackInfoParse {
  async fn parse(value: String) -> Option<TrackInfo>;
}

#[async_trait]
impl TrackInfoParse for TrackInfo {
  async fn parse(value: String) -> Option<TrackInfo> {
    RestTrackInfoParser::parse(value).await
  }
}
