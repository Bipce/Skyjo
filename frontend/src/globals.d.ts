declare global {
  interface Window {
    setHealth: (value: number) => void;
  }
}

export {};