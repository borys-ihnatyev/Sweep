import EventEmitter from "node:events";
import Path from "node:path";
import Chokidar from "chokidar";

const defaultAllowedExtensions = ["mp3", "m4a", "wav"];

export class WatcherService {
  private emitter = new EventEmitter();
  private watcher?: Chokidar.FSWatcher;

  get watchPattern() {
    return `${this.watchPath}/*.(${this.allowedExtensions.join("|")})`;
  }

  constructor(
    readonly watchPath: string,
    readonly allowedExtensions: ReadonlyArray<string> = defaultAllowedExtensions
  ) {}

  start(): void {
    if (this.watcher) {
      return;
    }

    this.watcher = Chokidar.watch(this.watchPattern, {
      ignoreInitial: true,
      depth: 0,
    }).on("add", this.onAdd);
  }

  stop(): void {
    if (!this.watcher) {
      return;
    }

    this.watcher.off("add", this.onAdd);
    this.watcher.close();
    this.watcher = undefined;
  }

  addListener(cb: (parsedPath: Path.ParsedPath) => void): VoidFunction {
    this.emitter.on("add", cb);

    return () => {
      this.emitter.off("add", cb);
    };
  }

  private onAdd = (filePath: string): void => {
    const parsedPath = Path.parse(filePath);
    this.emitter.emit("add", parsedPath);
  };
}
