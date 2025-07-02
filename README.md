**Authors:**
Górska Aleksandra
Kogutowska Amelia
Magrian Klaudia
Szczepańska Amelia

Obecnie w pliku client.ts klucze Supabase (supabaseUrl i supabaseAnonKey) są wpisane na sztywno w kodzie, co prowadzi do problemów z konfiguracją oraz potencjalnym problemem bezpieczeństwa.

Proponowana zmiana:

W pliku client.ts pobierać wartości z zmiennych środowiskowych:

```const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL;
const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;```
oraz wymagać, by te zmienne były ustawione:

lokalnie w pliku .env.local

oraz w konfiguracji środowiska na serwerze (np. w panelu hostingu).
