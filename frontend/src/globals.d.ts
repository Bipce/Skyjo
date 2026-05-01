import type { PlayerData } from "./interfaces/PlayerData.ts";
import type { CardData } from "./interfaces/CardData.ts";

declare global {
  interface Window {
    startNetwork: () => void;
    addPlayer: (player: PlayerData) => void;
    removePlayer: (id: number) => void;
    updatePlayer: (id: number, data: PlayerData) => void;
    selectCard: (playerId: number, id: number) => void;
    dropCard: (playerId: number, sourceId: number, targetId: number) => void;
    updateDrawnCard: (card: CardData) => void;
    updateDiscardedCard: (card: CardData) => void;
    roundOver: (isGameOver: boolean) => void;
  }
}

export {};
