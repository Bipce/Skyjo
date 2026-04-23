export interface CardData {
  id: number;
  number: number;
  isRevealed: boolean;
  isSelected: boolean;
}

export type CardBelongToType = "player" | "opponent" | "deck";
