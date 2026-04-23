import type { PlayerData } from "./interfaces/PlayerData.ts";
import type { CardData } from "./interfaces/CardData.ts";

declare global {
  interface Window {
    startNetwork: () => void;
    addPlayer: (value: PlayerData) => void;
    removePlayer: (username: string) => void;
    updatePlayer: (id: number, playerData: PlayerData) => void;
    selectCard: (playerId: number, cardId: number) => void;
    updateDrawnCard: (card: CardData) => void;
    updateDiscardedCard: (card: CardData) => void;
  }
}

export {};
