pub mod rest;

use async_trait::async_trait;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
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
pub trait TrackInfoService {
  async fn parse(value: String) -> Option<TrackInfo>;
}
