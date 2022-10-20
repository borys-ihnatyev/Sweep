mod track_info;
mod messaging;

use simple_logger::SimpleLogger;
use tokio::spawn;
use messaging::{MessagingService, events::{ListenEvent, EmitEvent}};
use notify::{Watcher, RecursiveMode};

#[tokio::main]
async fn main() {
    SimpleLogger::new()
        .without_timestamps()
        .with_level(log::LevelFilter::Trace)
        .init()
        .unwrap();

    let messaging_service = MessagingService::default();
    let mut watcher = notify::recommended_watcher(|event| {
        match event {
            Ok(event) => {
                log::info!("FS event {:?}", event)
            },
            Err(error) => log::error!("FS handler error {:#?}", error)
        }
    }).unwrap();

    let watch_path = home::home_dir().unwrap().join("Downloads");
    watcher.watch(&watch_path, RecursiveMode::NonRecursive).unwrap();

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
