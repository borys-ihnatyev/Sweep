use serde::{Serialize, Deserialize};
use reqwest::{Client};
use async_trait::async_trait;
use super::{TrackInfo, TrackInfoParse};

#[derive(Debug, Serialize)]
struct ParseRequest {
    value: String,
}

#[derive(Debug, Deserialize)]
struct ParseResponse {
    data: Option<TrackInfo>,
    error: Option<String>,
}

pub struct RestTrackInfoParser;

impl RestTrackInfoParser {
  async fn parse_request(value: String) -> Result<ParseResponse, reqwest::Error> {
    let body = ParseRequest {
        value,
    };

    Ok(
        Client::new()
            .post("http://localhost:5018/parse")
            .json(&body)
            .send()
            .await?
            .json()
            .await?
    )
  }
}

#[async_trait]
impl TrackInfoParse for RestTrackInfoParser {
  async fn parse(value: String) -> Option<TrackInfo> {
    let result = Self::parse_request(value).await;
    match result {
        Ok(response) =>
            match response.data {
                Some(info) => Some(info),
                None => {
                  let error = response.error.unwrap_or("".to_owned());
                  log::info!("Parse error response: {}", error);
                  None
                }
            },
        Err(error) => {
          log::error!("Parse request error: {:?}", error);
          None
        }
    }
  }
}
