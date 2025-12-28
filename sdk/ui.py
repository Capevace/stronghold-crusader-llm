import dash
from dash import dcc, html, callback, no_update
from dash.dependencies import Input, Output
import plotly.express as px
import pandas as pd
import logging

from flask import request

from .loop import GameStateStore
from .state import json_to_game_state


# Suppress werkzeug logs for a cleaner console
log = logging.getLogger('werkzeug')
log.setLevel(logging.ERROR)

def create_app(store: GameStateStore):
    """Creates and configures the Dash application."""
    app = dash.Dash(__name__)

    @app.server.route('/gamestate', methods=['POST'])
    def gamestate_endpoint():
        """Receives game state updates from the mod."""
        state_data = request.get_json()
        if state_data:
            try:
                game_state = json_to_game_state(state_data)
                store.add_state(game_state)
                return {"status": "success"}, 200
            except Exception as e:
                log.error(f"Error processing game state: {e}")
                return {"status": "error", "message": str(e)}, 400
        return {"status": "error", "message": "No data received"}, 400

    # Define the layout of the dashboard
    app.layout = html.Div(style={'backgroundColor': '#1E1E1E', 'color': '#E0E0E0', 'font-family': 'sans-serif'}, children=[
        html.H1("Crusade Compile Dashboard", style={'textAlign': 'center', 'color': '#4CAF50'}),

        dcc.Interval(
            id='interval-component',
            interval=1 * 1000,  # Refresh every second
            n_intervals=0
        ),

        html.Div(id='charts-container')
    ])

    @app.callback(
        Output('charts-container', 'children'),
        [Input('interval-component', 'n_intervals')]
    )
    def update_graphs(n):
        history = store.get_history()
        latest_state = store.latest_state

        if not history or not latest_state:
            return html.Div("Waiting for data...", style={'textAlign': 'center', 'padding': '20px'})

        df = pd.DataFrame(history)
        
        # --- Prepare data for charts ---
        template = 'plotly_dark'

        # --- Popularity ---
        pop_df = pd.json_normalize(df['popularity']).assign(time=df['time'])
        pop_line_fig = px.line(pop_df, x='time', y=[c for c in pop_df.columns if c != 'time'],
                               title="Popularity Factors Over Time", template=template)
        
        latest_pop_df = pd.DataFrame([latest_state.popularity.dict()])
        pop_bar_fig = px.bar(latest_pop_df, orientation='h', title="Current Popularity Factors", template=template,
                             labels={'value': 'Popularity Value', 'index': 'Factors'})
        pop_bar_fig.update_layout(yaxis_title=None, xaxis_title=None, showlegend=False)

        # --- Gold ---
        gold_df = pd.json_normalize(df['gold']).assign(time=df['time'])
        gold_line_fig = px.line(gold_df, x='time', y=[c for c in gold_df.columns if c != 'time'],
                                title="Gold Over Time", template=template)

        # --- Population ---
        population_df = pd.json_normalize(df['population']).assign(time=df['time'])
        population_line_fig = px.line(population_df, x='time', y=[c for c in population_df.columns if c != 'time'],
                                       title="Population Over Time", template=template)

        # --- Religion ---
        religion_df = pd.json_normalize(df['religion']).assign(time=df['time'])
        religion_line_fig = px.line(religion_df, x='time', y=[c for c in religion_df.columns if c != 'time'],
                                    title="Religion Over Time", template=template)
        
        # --- Economy ---
        economy_df = pd.json_normalize(df['economy']).assign(time=df['time'])
        economy_line_fig = px.line(economy_df, x='time', y=['efficiency'], title="Economy Efficiency Over Time", template=template)

        charts = [
            html.H2("Popularity", style={'textAlign': 'center'}),
            dcc.Graph(figure=pop_line_fig),
            dcc.Graph(figure=pop_bar_fig),
            html.H2("Economy & Resources", style={'textAlign': 'center'}),
            dcc.Graph(figure=gold_line_fig),
            dcc.Graph(figure=population_line_fig),
            dcc.Graph(figure=religion_line_fig),
            dcc.Graph(figure=economy_line_fig),
        ]

        return charts
    
    return app

def render(store: GameStateStore):
    """Starts the Dash web server to display the dashboard."""
    if not isinstance(store, GameStateStore):
        raise TypeError("The render function requires a GameStateStore instance.")
    
    app = create_app(store)
    print("Starting dashboard server at http://127.0.0.1:8050/")
    app.run(debug=True, use_reloader=False)