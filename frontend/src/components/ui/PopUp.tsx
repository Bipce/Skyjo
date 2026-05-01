import type { PopupEvent } from "../../interfaces/Popup.ts";

interface Props {
  event: PopupEvent | null;
}

const POPUP_CONFIG: Record<PopupEvent, { text: string; className: string }> = {
  your_turn: { text: "C'est ton tour !", className: "bg-sky-500" },
  round_win: { text: "Tu gagnes la manche !", className: "bg-green-600" },
  round_lose: { text: "Tu perds la manche...", className: "bg-red-500" },
  game_win: { text: "Tu gagnes la partie !", className: "bg-emerald-600" },
  game_lose: { text: "Tu perds la partie...", className: "bg-red-700" },
  double_penalty: { text: "Tu te prends x2 !", className: "bg-orange-700" },
};

const PopUp = ({ event }: Props) => {
  if (!event) return null;
  const { className, text } = POPUP_CONFIG[event];

  return (
    <div
      className={`absolute top-0 left-1/2 z-10 w-full max-w-36 -translate-x-1/2 rounded-xl p-3 text-center text-base ${className}`}
    >
      {text}
    </div>
  );
};

export default PopUp;
