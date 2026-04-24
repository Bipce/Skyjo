import type { CardData } from "./CardData.ts";

export interface PlayerData {
  id: number;
  username: string;
  isOwner: boolean;
  cards: CardData[];
  currentScore: number;
  totalScore: number;
  isCurrentPlayer: boolean;
}
