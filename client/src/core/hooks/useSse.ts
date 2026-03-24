import { useRef, useState, useCallback } from "react";

export type SseStatus = "idle" | "connecting" | "connected" | "error" | "closed";

type SseEventHandlers = {
  [eventName: string]: (event: MessageEvent) => void;
};

interface UseSseOptions {
  url: string;
  onOpen?: () => void;
  onError?: (error: Event) => void;
  events?: SseEventHandlers; // named event handlers
}

export function useSse({ url, onOpen, onError, events = {} }: UseSseOptions) {
  const [status, setStatus] = useState<SseStatus>("idle");
  const eventSourceRef = useRef<EventSource | null>(null);
  const eventsRef = useRef<SseEventHandlers>(events);
  eventsRef.current = events;

  const connect = useCallback(() => {
    if (eventSourceRef.current) return;
    console.log("connect() called, registered events:", Object.keys(eventsRef.current));
    setStatus("connecting");

    const es = new EventSource(url, { withCredentials: true });
    eventSourceRef.current = es;

    es.onopen = () => {
      setStatus("connected");
      onOpen?.();
    };

    es.onerror = (error) => {
      setStatus("error");
      onError?.(error);
      if (es.readyState === EventSource.CLOSED) setStatus("closed");
    };

    Object.keys(eventsRef.current).forEach((eventName) => {
      es.addEventListener(eventName, (event) => {
        eventsRef.current[eventName]?.(event as MessageEvent);
      });
    });
  }, [url]);

  const close = useCallback(() => {
    if (eventSourceRef.current) {
      eventSourceRef.current.close();
      eventSourceRef.current = null;
      setStatus("closed");
    }
  }, []);

  return {
    status,
    close,
    connect,
    reconnect: () => { close(); connect(); },
  };
}