declare global {
  interface Window {
    startNetwork: () => void;
    setUsername: (value: string) => void;
  }
}

export {};