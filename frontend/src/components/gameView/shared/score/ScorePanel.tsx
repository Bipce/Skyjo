import { Trophy, Hourglass } from "lucide-react";
import IconWrapper from "../../../ui/IconWrapper.tsx";
import type { PlayerData } from "../../../../interfaces/PlayerData.ts";

interface Props {
  player: PlayerData;
}

const ScorePanel = ({ player }: Props) => {
  const { username, currentScore, totalScore, hasDoublePoint } = player;

  return (
    <aside className="w-32 shrink-0 text-lg">
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
