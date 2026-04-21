import type { PlayerData } from "./interfaces/PlayerData.ts";

declare global {
  interface Window {
    startNetwork: () => void;
    addPlayer: (value: PlayerData) => void;
    removePlayer: (username: string) => void;
  }
}

export {};
