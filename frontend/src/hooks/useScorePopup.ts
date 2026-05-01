import { useEffect, useState } from "react";
import { useGameStore } from "../store/gameStore.ts";
import { useShallow } from "zustand/react/shallow";
import type { PopupEvent } from "../interfaces/Popup.ts";
import type { PlayerData } from "../interfaces/PlayerData.ts";

export const useScorePopup = (
  isCurrentPlayer: boolean,
  hasDoublePoint: boolean,
  players: PlayerData[],
  currentScore: number,
  totalScore: number,
  isOwner: boolean,
) => {
  const [popupEvent, setPopupEvent] = useState<PopupEvent | null>(null);
  const { clearRoundOverEvent, roundOverEvent } = useGameStore(
    useShallow(s => ({
      clearRoundOverEvent: s.clearRoundOverEvent,
      roundOverEvent: s.roundOverEvent,
    })),
  );

  const showPopup = (event: PopupEvent) => {
    setPopupEvent(event);
    const timeout = setTimeout(() => setPopupEvent(null), 2000);
    return () => clearTimeout(timeout);
  };

  useEffect(() => {
    if (!isCurrentPlayer || !isOwner) return;
    const timeout = setTimeout(() => showPopup("your_turn"), 0);
    return () => clearTimeout(timeout);
  }, [isCurrentPlayer, isOwner]);

  useEffect(() => {
    if (!hasDoublePoint || !isOwner) return;
    const timeout = setTimeout(() => showPopup("double_penalty"), 0);
    return () => clearTimeout(timeout);
  }, [hasDoublePoint, isOwner]);

  useEffect(() => {
    if (!roundOverEvent || !isOwner) return;

    const getScoreEvent = (type: "round" | "game"): PopupEvent | null => {
      if (type === "game" && !players.some(p => p.totalScore >= 100)) return null;

      const scores = players.map(p => (type === "round" ? p.currentScore : p.totalScore));
      const playerScore = type === "round" ? currentScore : totalScore;
      const min = Math.min(...scores);
      const max = Math.max(...scores);

      if (playerScore === min) return type === "round" ? "round_win" : "game_win";
      if (playerScore === max) return type === "round" ? "round_lose" : "game_lose";
      return null;
    };

    const event = roundOverEvent.isGameOver ? getScoreEvent("game") : getScoreEvent("round");
    console.log("roundOverEvent", roundOverEvent);
    if (!event) return;

    const timeout = setTimeout(() => {
      showPopup(event);
      clearRoundOverEvent();
    }, 0);
    return () => clearTimeout(timeout);
  }, [clearRoundOverEvent, currentScore, players, roundOverEvent, totalScore, isOwner]);

  return { popupEvent, showPopup };
};
