import { useShallow } from "zustand/react/shallow";
import { DragDropProvider, PointerSensor } from "@dnd-kit/react";
import { PointerActivationConstraints } from "@dnd-kit/dom";
import { useGameStore } from "../store/gameStore.ts";
import OpponentsGrid from "../components/gameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/gameView/opponent/OpponentPanel.tsx";
import CardDeck from "../components/gameView/shared/card/CardDeck.tsx";
import PlayerPanel from "../components/gameView/player/PlayerPanel.tsx";
import Overlay from "../components/ui/Overlay.tsx";

const SENSORS = [
  PointerSensor.configure({
    activationConstraints: [new PointerActivationConstraints.Distance({ value: 8 })],
    preventActivation: () => false,
  }),
];

const GameView = () => {
  const { players, player, drawnCard, discardedCard, dropCard, setHasDiscardedDrawnCard } = useGameStore(
    useShallow(s => ({
      players: s.players,
      player: s.player,
      drawnCard: s.drawnCard,
      discardedCard: s.discardedCard,
      dropCard: s.dropCard,
      setHasDiscardedDrawnCard: s.setHasDiscardedDrawnCard,
    })),
  );

  return (
    <DragDropProvider
      sensors={SENSORS}
      onDragEnd={event => {
        if (event.canceled || !event.operation.target) return;

        const sourceCardId = Number(event.operation.source?.id);
        const targetCardId = Number(event.operation.target.id);

        if (drawnCard?.id === sourceCardId && discardedCard?.id === targetCardId) setHasDiscardedDrawnCard(true);

        dropCard(sourceCardId, targetCardId);
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

      <Overlay drawnCard={drawnCard} discardedCard={discardedCard} />
    </DragDropProvider>
  );
};

export default GameView;
