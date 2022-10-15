
use std::sync::{Arc};

use tokio::{net::{TcpListener, TcpStream}, spawn, task::JoinHandle, select, sync::broadcast::error::SendError};
use tokio::io::{AsyncBufReadExt, AsyncWriteExt, BufReader};
use tokio::sync::broadcast::{Sender, channel};

struct State {
  read_sender: Arc<Sender<String>>,
  write_sender: Arc<Sender<String>>,
}

impl State {
  fn new() -> State {
    let read = channel(10);
    let write = channel(10);

    State {
      read_sender: Arc::new(read.0),
      write_sender: Arc::new(write.0),
    }
  }
}

pub struct MessagingService {
  address: String,
  state: Arc<State>,
}

impl MessagingService {
  pub fn new(host: &str, port: usize) -> MessagingService {
    MessagingService {
      address: format!("{}:{}", host, port),
      state: Arc::new(State::new()),
    }
  }

  pub fn on_message<Handler>(self, handler: Handler) -> Self
  where Handler: Fn(Arc<Sender<String>>, String) + Send + 'static {
    let sender = Arc::clone(&self.state.read_sender);
    let state = Arc::clone(&self.state);

    spawn(async move {
      let mut receiver = sender.subscribe();
      while let Ok(message) = receiver.recv().await {
        let sender = Arc::clone(&state.write_sender);
        handler(sender, message);
      }
    });

    self
  }

  pub fn send(&self, msg: String) -> Result<(), SendError<String>> {
    let state = Arc::clone(&self.state);
    state.write_sender.send(msg).and(Ok(()))
  }

  pub fn default() -> MessagingService {
    Self::new("127.0.0.1", 3333)
  }

  pub fn start(&self) -> JoinHandle<()> {
    let address = self.address.clone();
    let state = Arc::clone(&self.state);

    spawn(async move {
      let listener = TcpListener::bind(address).await.unwrap();

      loop {
        match listener.accept().await {
          Ok((stream, _)) => {
            let state = Arc::clone(&state);
            spawn(Self::handle_connection(stream, state));
          },
          Err(e) => {
            log::error!("couldn't get client: {:?}", e);
          },
        }
      }
    })
  }

  async fn handle_connection(mut stream: TcpStream, state: Arc<State>) -> Option<()> {
    let (reader, mut writer) = stream.split();
    let mut reader = BufReader::new(reader);
    let sender = Arc::clone(&state.read_sender);
    let mut receiver = state.write_sender.subscribe();

    loop {
        let mut line = String::new();

        select! {
          result = reader.read_line(&mut line) => {
            result.unwrap();
            sender.send(line).unwrap();
          }
          result = receiver.recv() => {
            let message = result.unwrap();
            writer.write_all(message.as_bytes()).await.unwrap();
          }
        }
    }
  }

}
