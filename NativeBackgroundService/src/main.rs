mod track_info;
mod messaging;

use simple_logger::SimpleLogger;
use tokio::spawn;
use track_info::{TrackInfo, TrackInfoParse};
use messaging::{MessagingService, events::{ListenEvent, EmitEvent}};

#[tokio::main]
async fn main() {
    SimpleLogger::new()
        .without_timestamps()
        .with_level(log::LevelFilter::Trace)
        .init()
        .unwrap();

    let messaging_service = MessagingService::default();
    messaging_service
        .listen(|_, event| {
            spawn(async move {
                match event {
                    ListenEvent::ConfirmEdit(response) => {
                        log::info!("Confirm edit response {:#?}", response);
                    },
                    ListenEvent::WatchToggle { enabled } => {
                        log::info!("Watch enabled {}", enabled);
                    }
                }
            });
        })
        .start()
        .await.unwrap()
}
