export interface ICard {
  number: number;
  isRevealed: boolean;
  belongTo: CardBelongToType;
}

export type CardBelongToType = "player" | "opponent" | "deck";
