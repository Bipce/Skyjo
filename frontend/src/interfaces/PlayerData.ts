import type { CardData } from "./CardData.ts";

export interface PlayerData {
  id: number;
  username: string;
  isOwner: boolean;
  cards: CardData[];
}
