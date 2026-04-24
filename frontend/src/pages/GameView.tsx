import { useShallow } from "zustand/react/shallow";
import { DragDropProvider, DragOverlay, PointerSensor } from "@dnd-kit/react";
import { PointerActivationConstraints } from "@dnd-kit/dom";
import type { Draggable } from "@dnd-kit/dom";
import OpponentsGrid from "../components/gameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/gameView/opponent/OpponentPanel.tsx";
import CardDeck from "../components/gameView/shared/card/CardDeck.tsx";
import Card from "../components/gameView/shared/card/Card.tsx";
import PlayerPanel from "../components/gameView/player/PlayerPanel.tsx";
import { useGameStore } from "../store/gameStore.ts";

const SENSORS = [
  PointerSensor.configure({
    activationConstraints: [new PointerActivationConstraints.Distance({ value: 5 })],
    preventActivation: () => false,
  }),
];

const GameView = () => {
  const { players, player, drawnCard, discardedCard } = useGameStore(
    useShallow(s => ({
      players: s.players,
      player: s.player,
      drawnCard: s.drawnCard,
      discardedCard: s.discardedCard,
    })),
  );

  return (
    <DragDropProvider
      sensors={SENSORS}
      onDragEnd={event => {
        if (event.canceled || !event.operation.target) return;

        const sourceId = Number(event.operation.source?.id);
        const targetId = Number(event.operation.target.id);
      }}
    >
      <main className="grid min-h-screen place-items-center py-10">
        <div className="grid w-full max-w-6xl gap-8">
          <OpponentsGrid>
            {players
              .filter(player => !player.isOwner)
              .map(player => {
                return <OpponentPanel key={player.id} player={player} />;
              })}
          </OpponentsGrid>

          {drawnCard && discardedCard && <CardDeck drawnCard={drawnCard} discardedCard={discardedCard} />}

          {player && <PlayerPanel player={player} />}
        </div>
      </main>

      <DragOverlay>
        {(source: Draggable) => {
          const card = [drawnCard, discardedCard].find(c => c?.id === Number(source.id));
          return card ? <Card card={card} belongsTo="deck" /> : null;
        }}
      </DragOverlay>
    </DragDropProvider>
  );
};

export default GameView;
