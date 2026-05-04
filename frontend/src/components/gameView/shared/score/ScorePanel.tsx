import { Trophy, Hourglass } from "lucide-react";
import { useScorePopup } from "../../../../hooks/useScorePopup.ts";
import IconWrapper from "../../../ui/IconWrapper.tsx";
import PopUp from "../../../ui/PopUp.tsx";
import type { PlayerData } from "../../../../interfaces/PlayerData.ts";

interface Props {
  player: PlayerData;
}

const ScorePanel = ({ player }: Props) => {
  const { username, currentScore, totalScore, hasDoublePoint } = player;
  const { popupEvent } = useScorePopup(player);

  return (
    <aside className="relative flex h-full w-32 shrink-0 flex-col justify-center text-lg">
      {popupEvent && <PopUp event={popupEvent} />}
      <p className="mb-8 text-2xl font-bold">{username}</p>

      <div className="flex flex-col gap-3">
        <IconWrapper>
          <Hourglass className="text-purple-400" /> :
          <span className={`${hasDoublePoint && "text-red-500"}`}>
            {currentScore} {hasDoublePoint && <span className="text-sm italic">({currentScore * 2})</span>}
          </span>
        </IconWrapper>

        <IconWrapper>
          <Trophy className="text-yellow-400" /> : {totalScore} / 100
        </IconWrapper>
      </div>
    </aside>
  );
};

export default ScorePanel;
