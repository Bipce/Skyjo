import { useState } from "react";
import { useDragDropMonitor } from "@dnd-kit/react";

export const useDragActive = (): boolean => {
  const [isDragActive, setIsDragActive] = useState(false);
  useDragDropMonitor({
    onDragStart: () => setIsDragActive(true),
    onDragEnd: () => setIsDragActive(false),
  });
  return isDragActive;
};
