import { create } from "zustand";
import type { PlayerData } from "../interfaces/PlayerData.ts";
import type { CardData } from "../interfaces/CardData.ts";

interface GameState {
  players: PlayerData[];
  player: PlayerData | null;
  drawnCard: CardData | null;
  discardedCard: CardData | null;
}

interface GameCallbacks {
  addPlayer: (player: PlayerData) => void;
  removePlayer: (id: number) => void;
  updatePlayer: (id: number, data: PlayerData) => void;
  updateDrawnCard: (card: CardData) => void;
  updateDiscardedCard: (card: CardData) => void;
  roundOver: (isGameOver: boolean) => void;
  bindWindowCallbacks: () => void;
}

interface GameCommands {
  selectCard: (id: number) => void;
  dropCard: (sourceId: number, targetId: number) => void;
  startNetwork: () => void;
}

interface GameUI {
  hasDiscardedDrawnCard: boolean;
  roundOverEvent: {
    isGameOver: boolean;
    id: number;
  } | null;
  setHasDiscardedDrawnCard: (value: boolean) => void;
  hasGameStarted: () => boolean;
}

let roundOverEventId = 0;

const isRoundSetupPlayer = (player: PlayerData) =>
  !player.isCurrentPlayer &&
  player.currentScore === 0 &&
  !player.hasDoublePoint &&
  player.cards.every(card => !card.isRevealed && !card.isSelected && !card.isHighlighted);

export const useGameStore = create<GameState & GameCallbacks & GameCommands & GameUI>((set, get) => ({
  players: [],
  player: null,
  drawnCard: null,
  discardedCard: null,

  addPlayer: player =>
    set(state => ({
      players: [...state.players, player],
      player: player.isOwner ? player : state.player,
    })),

  removePlayer: id =>
    set(state => ({
      players: state.players.filter(p => p.id !== id),
    })),

  updatePlayer: (id, playerData) =>
    set(state => {
      const players = [...state.players];
      const index = players.findIndex(p => p.id === id);
      players[index] = playerData;

      const hasRoundBeenCleaned = state.roundOverEvent && isRoundSetupPlayer(playerData);

      return {
        players,
        player: playerData.isOwner ? playerData : state.player,
        hasDiscardedDrawnCard: false,
        roundOverEvent: hasRoundBeenCleaned ? null : state.roundOverEvent,
      };
    }),

  updateDrawnCard: card => set({ drawnCard: card }),
  updateDiscardedCard: card => set({ discardedCard: card }),

  roundOverEvent: null,
  roundOver: isGameOver =>
    set({
      roundOverEvent: { isGameOver, id: ++roundOverEventId },
    }),

  hasDiscardedDrawnCard: false,
  setHasDiscardedDrawnCard: value => set({ hasDiscardedDrawnCard: value }),

  selectCard: cardId => {
    const player = get().player;
    if (!player) return null;

    window.selectCard(player.id, cardId);
  },

  dropCard: (sourceId, targetId) => {
    const player = get().player;
    if (!player) return null;
    window.dropCard(player.id, sourceId, targetId);
  },

  startNetwork: () => {
    window.startNetwork();
  },

  hasGameStarted: () => {
    const players = get().players;
    return players.some(player => player.isCurrentPlayer);
  },

  bindWindowCallbacks: () => {
    const { addPlayer, removePlayer, updatePlayer, updateDrawnCard, updateDiscardedCard, roundOver } =
      useGameStore.getState();

    window.addPlayer = addPlayer;
    window.removePlayer = removePlayer;
    window.updatePlayer = updatePlayer;
    window.updateDrawnCard = updateDrawnCard;
    window.updateDiscardedCard = updateDiscardedCard;
    window.roundOver = roundOver;
  },
}));
