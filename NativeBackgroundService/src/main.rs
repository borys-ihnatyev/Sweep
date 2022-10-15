mod track_info;
mod messaging;

use simple_logger::SimpleLogger;
use tokio::spawn;
use track_info::{rest::RestTrackInfoService, TrackInfoService};
use messaging::MessagingService;

#[tokio::main]
async fn main() {
    SimpleLogger::new()
        .without_timestamps()
        .with_level(log::LevelFilter::Trace)
        .init()
        .unwrap();

    MessagingService::default()
        .on_message(|sender, msg| {
            spawn(async move {
                let result = RestTrackInfoService::parse(msg).await;
                let msg = serde_json::to_string_pretty(&result).unwrap();
                sender.send(msg).unwrap();
            });
        })
        .start().await.unwrap();
}
