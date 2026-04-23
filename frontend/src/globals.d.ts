import type { PlayerData } from "./interfaces/PlayerData.ts";
import type { CardData } from "./interfaces/CardData.ts";

declare global {
  interface Window {
    startNetwork: () => void;
    addPlayer: (value: PlayerData) => void;
    removePlayer: (id: number) => void;
    updatePlayer: (id: number, playerData: PlayerData) => void;
    selectCard: (cardId: number) => void;
    updateDrawnCard: (card: CardData) => void;
    updateDiscardedCard: (card: CardData) => void;
  }
}

export {};
