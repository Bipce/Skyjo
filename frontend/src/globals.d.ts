import type { PlayerData } from "./interfaces/PlayerData.ts";
import type { CardData } from "./interfaces/CardData.ts";

declare global {
  interface Window {
    startNetwork: () => void;
    addPlayer: (value: PlayerData) => void;
    removePlayer: (username: string) => void;
    updatePlayer: (username: string, playerData: PlayerData) => void;
    selectCard: (username: string, indexes: number[]) => void;
    updateDrawnCard: (card: CardData) => void;
    updateDiscardedCard: (card: CardData) => void;
  }
}

export {};
