export interface CardData {
  number: number;
  isRevealed: boolean;
  belongTo: CardBelongToType;
}

export type CardBelongToType = "player" | "opponent" | "deck";
