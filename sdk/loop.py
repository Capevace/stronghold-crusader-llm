from time import sleep
from .state import get_game_state
import datetime
from threading import Lock
from collections import deque

class GameStateStore:
    """A thread-safe store for game state history."""
    def __init__(self, max_history=200):
        self._history = deque(maxlen=max_history)
        self._lock = Lock()
        self.latest_state = None

    def add_state(self, state):
        """Adds a new game state to the history."""
        timestamped_state = {
            'time': datetime.datetime.now(),
            **state.dict()
        }
        with self._lock:
            self._history.append(timestamped_state)
            self.latest_state = state

    def get_history(self):
        """Returns the entire history of game states."""
        with self._lock:
            return list(self._history)

def state_update_loop(store: GameStateStore, state_file_path: str, interval_seconds: int = 1):
    """
    Continuously polls the game state file and updates the store.
    
    This loop is where you can add your own logic to react to state changes.
    """
    print("State update loop started. Watching for changes...")
    last_state_raw = None
    while True:
        try:
            # We get the raw state to avoid unnecessary parsing if nothing has changed
            current_state = get_game_state(state_file_path)
            
            # Simple object comparison works well for Pydantic models
            if current_state != store.latest_state:
                print(f"State changed at {datetime.datetime.now()}")
                store.add_state(current_state)
                # Here you could add custom logic, e.g.:
                # if store.latest_state.gold.current < 50:
                #     print("Warning: Gold is low!")

        except FileNotFoundError:
            print(f"Warning: State file not found at '{state_file_path}'. Waiting...")
        except Exception as e:
            print(f"Error in state update loop: {e}")
        
        sleep(interval_seconds) 