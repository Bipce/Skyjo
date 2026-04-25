import type { PlayerData } from "../interfaces/PlayerData.ts";
import type { CardData } from "../interfaces/CardData.ts";
import { create } from "zustand";

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
  bindWindowCallbacks: () => void;
}

interface GameCommands {
  selectCard: (id: number) => void;
  dropCard: (sourceId: number, targetId: number) => void;
  startNetwork: () => void;
}

export const useGameStore = create<GameState & GameCallbacks & GameCommands>((set, get) => ({
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
      return { players, player: playerData.isOwner ? playerData : state.player };
    }),

  updateDrawnCard: card => set({ drawnCard: card }),
  updateDiscardedCard: card => set({ discardedCard: card }),

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

  bindWindowCallbacks: () => {
    const { addPlayer, removePlayer, updatePlayer, updateDrawnCard, updateDiscardedCard } = useGameStore.getState();

    window.addPlayer = addPlayer;
    window.removePlayer = removePlayer;
    window.updatePlayer = updatePlayer;
    window.updateDrawnCard = updateDrawnCard;
    window.updateDiscardedCard = updateDiscardedCard;
  },
}));
