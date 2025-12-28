import argparse
from threading import Thread
from sdk.loop import GameStateStore, state_update_loop
from sdk.ui import render

def main():
    """
    Main entry point for the application.
    
    This function sets up the state management and can run with or without the UI.
    """
    parser = argparse.ArgumentParser(description="Crusade Compile - Game State Monitor")
    parser.add_argument(
        "--no-ui",
        action="store_true",
        help="Run the application in headless mode without the dashboard."
    )
    parser.add_argument(
        "--file",
        type=str,
        default="/mnt/c/Program Files (x86)/Steam/steamapps/common/Stronghold Crusader Definitive Edition/GameState.json",
        help="Path to the GameState.json file."
    )
    args = parser.parse_args()

    # Initialize the shared state store
    state_store = GameStateStore()

    # Start the state update loop in a background thread
    state_thread = Thread(
        target=state_update_loop,
        args=(state_store, args.file),
        daemon=True  # Allows the main thread to exit even if this thread is running
    )
    state_thread.start()

    # If the UI is enabled (default), start the Dash server
    if not args.no_ui:
        try:
            render(state_store)
        except Exception as e:
            print(f"Failed to start the dashboard: {e}")
            print("Running in headless mode. The state update loop is still active.")
            # Keep the main thread alive to allow the background thread to continue
            state_thread.join()
    else:
        print("Running in headless mode. The state update loop is active.")
        # Keep the main thread alive
        state_thread.join()

if __name__ == "__main__":
    main()
