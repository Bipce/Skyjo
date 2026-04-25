import { DragOverlay, useDragOperation } from "@dnd-kit/react";
import type { Draggable } from "@dnd-kit/dom";
import type { CardData } from "../../interfaces/CardData.ts";
import Card from "../gameView/shared/card/Card.tsx";

interface Props {
  drawnCard: CardData | null;
  discardedCard: CardData | null;
}

const Overlay = ({ drawnCard, discardedCard }: Props) => {
  const { target } = useDragOperation();
  return (
    <DragOverlay dropAnimation={target ? null : undefined}>
      {(source: Draggable) => {
        const card = [drawnCard, discardedCard].find(c => c?.id === Number(source.id));
        return card ? <Card card={card} belongsTo="deck" /> : null;
      }}
    </DragOverlay>
  );
};

export default Overlay;
