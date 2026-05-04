import { useEffect, useState } from "react";
import { useGameStore } from "../store/gameStore.ts";
import type { PopupEvent } from "../interfaces/Popup.ts";
import type { PlayerData } from "../interfaces/PlayerData.ts";

type ScorePopupPlayer = Pick<PlayerData, "id" | "isOwner" | "isCurrentPlayer" | "hasDoublePoint">;
type ScorePlayer = Pick<PlayerData, "id" | "currentScore" | "totalScore" | "hasDoublePoint">;

const getScoreEvent = (
  type: "round" | "game",
  playerId: number,
  players: ScorePlayer[],
): PopupEvent | null => {
  const player = players.find(player => player.id === playerId);
  if (!player || players.length === 0) return null;

  const scores = players.map(player => (type === "round" ? player.currentScore : player.totalScore));
  const playerScore = type === "round" ? player.currentScore : player.totalScore;
  const min = Math.min(...scores);
  const max = Math.max(...scores);

  if (playerScore === min) return type === "round" ? "round_win" : "game_win";
  if (playerScore === max && max !== min) return type === "round" ? "round_lose" : "game_lose";
  return null;
};

const getRoundPopupEvent = (playerId: number, players: ScorePlayer[]): PopupEvent | null => {
  const player = players.find(player => player.id === playerId);
  if (player?.hasDoublePoint) return "double_penalty";

  return getScoreEvent("round", playerId, players);
};

export const useScorePopup = (player: ScorePopupPlayer): { popupEvent: PopupEvent | null } => {
  const roundOverEvent = useGameStore(s => s.roundOverEvent);
  const players = useGameStore(s => s.players);
  const [readyRoundOverEventId, setReadyRoundOverEventId] = useState<number | null>(null);

  useEffect(() => {
    if (!roundOverEvent) return;

    const timeout = window.setTimeout(() => setReadyRoundOverEventId(roundOverEvent.id), 0);
    return () => window.clearTimeout(timeout);
  }, [roundOverEvent]);

  if (!player.isOwner) return { popupEvent: null };

  if (roundOverEvent) {
    const canReadFinalScores = readyRoundOverEventId === roundOverEvent.id;
    if (!canReadFinalScores) return { popupEvent: null };

    const popupEvent = roundOverEvent.isGameOver
      ? getScoreEvent("game", player.id, players)
      : getRoundPopupEvent(player.id, players);

    return { popupEvent };
  }

  if (player.hasDoublePoint) return { popupEvent: "double_penalty" };
  if (player.isCurrentPlayer) return { popupEvent: "your_turn" };

  return { popupEvent: null };
};
