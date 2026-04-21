import type { CardData } from "./CardData.ts";

export interface PlayerData {
  username: string;
  isOwner: boolean;
  cards: CardData[];
}