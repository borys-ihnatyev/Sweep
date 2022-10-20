
use serde::{Serialize, Deserialize};

use crate::track_info::{TrackInfo};

#[derive(Clone, Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
#[serde(tag = "type", content = "data")]
pub enum ListenEvent {
  ConfirmEdit(ConfirmEditResponse),
  WatchToggle { enabled: bool}
}

#[derive(Clone, Debug, Serialize)]
#[serde(rename_all = "camelCase")]
#[serde(tag = "type", content = "data")]
pub enum EmitEvent {
  ConfirmEdit(ConfirmEditRequest)
}

#[derive(Clone, Debug, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct ConfirmEditRequest {
  pub id: u32,
  pub location: FileLocation,
  pub track_info: TrackInfo,
}

#[derive(Clone, Debug, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct FileLocation {
  pub parsed_path: Option<ParsedPath>,
  pub path: String,
}

#[derive(Clone, Debug, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct ParsedPath {
  /**
   * The root of the path such as '/' or 'c:\'
  */
  pub root: String,

  /**
   * The full directory path such as '/home/user/dir' or 'c:\path\dir'
   */
  pub dir: String,

  /**
   * The file name including extension (if any) such as 'index.html'
   */
  pub base: String,

  /**
   * The file extension (if any) such as '.html'
   */
  pub ext: String,

  /**
   * The file name without extension (if any) such as 'index'
   */
  pub name: String,
}

#[derive(Clone, Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ConfirmEditResponse {
  pub request_id: String,
  pub resolution: ConfirmEditResponseResolution,
}

#[derive(Clone, Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
#[serde(tag = "type")]
pub enum ConfirmEditResponseResolution {
  Allow,
  Deny,
  #[serde(rename_all = "camelCase")]
  Rename { new_name: String }
}
