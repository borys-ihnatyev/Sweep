mod rest_track_info_parser;

use std::{collections::HashMap, path::PathBuf, sync::{Mutex, atomic::{AtomicU32, Ordering}}};

use async_trait::async_trait;
use serde::{Deserialize, Serialize};

use crate::messaging::event::{ConfirmEditRequest, FileLocation};

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

pub struct TrackInfoService {
  last_id: AtomicU32,
  pending_requests: Mutex<HashMap<u32, ConfirmEditRequest>>,
}

impl TrackInfoService {
  pub fn new() -> TrackInfoService {
    TrackInfoService {
      last_id: AtomicU32::new(1),
      pending_requests: Mutex::new(HashMap::new())
    }
  }

  pub async fn create_edit_request(&mut self, path: PathBuf) -> Option<ConfirmEditRequest> {
    let id = self.next_id();
    let request = create_edit_request(id, path).await?;
    let mut pending_requests = self.pending_requests.lock().ok()?;
    let result = request.clone();
    pending_requests.insert(request.id, request)?;
    Some(result)
  }

  fn next_id(&mut self) -> u32 {
    self.last_id.fetch_add(1, Ordering::Relaxed)
  }
}

async fn create_edit_request(id: u32, path: PathBuf) -> Option<ConfirmEditRequest> {
  let file_name = path.as_path().file_name()?.to_str()?;
  let track_info = TrackInfo::parse(file_name.to_owned()).await?;

  Some(
    ConfirmEditRequest {
    id,
    location: FileLocation {
      path: path.to_str()?.to_owned(),
      parsed_path: None,
    },
    track_info,
  })
}
