import { useEffect } from "react";
import { useAtomValue, useSetAtom } from "jotai";
import { authAtom } from "../../core/atoms/authAtom";
import { notificationsAtom } from "../../core/atoms/notificationAtom";
import { useSse } from "../../core/hooks/useSse";
import { notificationApi } from "../../core/controllers/notificationApi";
import type { Notification } from "../../core/types/Notification";

const SseConnection: React.FC = () => {
  const auth = useAtomValue(authAtom);
  const setNotifications = useSetAtom(notificationsAtom);

  useEffect(() => {
    if (auth.status !== "authenticated") return;
    notificationApi.getUnread()
        .then(setNotifications)
        .catch(console.error);
  }, [auth.status]);

  console.log("SseConnection render, auth status: ", auth.status, "");
  const sse = useSse({
    url: "/api/chat/stream",
    onError: (error) => console.error("SSE error:", error),
    events: {
      ping: () => {},
      connected: (event) => console.log("SSE connected:", event.data),

      notification: (event) => {
        console.log("SSE notification:", event.data);
        const data = JSON.parse(event.data);

        if (data.type === "poke") {
          const notification: Notification = {
            id: data.notificationId,
            type: "Poke",
            payload: event.data,
            isRead: false,
            createdAt: data.createdAt ?? new Date().toISOString(),
          };
          setNotifications((prev) => [notification, ...prev]);
        }

        if (data.type === "new_message") {
          const notification: Notification = {
            id: data.notificationId,
            type: "NewMessage",
            payload: event.data,
            isRead: false,
            createdAt: new Date().toISOString(),
          };
          setNotifications((prev) => [notification, ...prev]);
        }
      },
    },
  });

  useEffect(() => {
    if (auth.status === "authenticated") {
      console.log("calling sse.connect()");
      sse.connect();
    } else {
      sse.close();
    }
  }, [auth.status]);

  return null;
};

export default SseConnection;