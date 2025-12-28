import json
from pydantic import BaseModel


class Popularity(BaseModel):
    current: int
    upcoming: int
    food: int
    tax: int
    overcrowding: int
    religion: int
    fairs: int
    plague: int
    wolves: int
    bandits: int
    fire: int
    marriage: int
    jester: int
    fear: int
    good_things: int
    bad_things: int

class Population(BaseModel):
    current: int
    max: int
    available_for_troops: int
    rationing: int

class Food(BaseModel):
    clock: int
    total: int
    months_of_food: int
    types_eaten: int
    types_available: int

class Audio(BaseModel):
    # nullable string
    speech: str | None
    music: str | None
    bink: str | None

class Gold(BaseModel):
    current: int
    tax_rate: int
    tax_amount: int

class Religion(BaseModel):
    num_priests: int
    blessed_percent: int
    blessed_next_level_at: int

class Economy(BaseModel):
    efficiency: int



class GameState(BaseModel):
    time: int
    popularity: Popularity
    population: Population
    gold: Gold
    food: Food
    audio: Audio
    religion: Religion
    economy: Economy
    
    fear_factor: int
    upcoming_fear_factor: int

def get_game_state(filepath: str) -> GameState:
    return json_to_game_state(read_state_from_file(filepath))

def read_state_from_file(filepath: str) -> dict:
    with open(filepath, "r") as f:
        return json.load(f)

def json_to_game_state(json_data: dict) -> GameState:
    return GameState(
        time=json_data["game_time"],
        popularity=Popularity(
            current=json_data["popularity"],
            upcoming=json_data["upcoming_total_popularity"],
            food=json_data["food_popularity"],
            tax=json_data["tax_popularity"],
            overcrowding=json_data["overcrowding_popularity"],
            religion=json_data["religion_popularity"],
            fairs=json_data["fairs_popularity"],
            plague=json_data["plague_popularity"],
            wolves=json_data["wolves_popularity"],
            bandits=json_data["bandits_popularity"],
            fire=json_data["fire_popularity"],
            marriage=json_data["marriage_popularity"],
            jester=json_data["jester_popularity"],
            fear=json_data["fearFactor_popularity"],
            good_things=json_data["good_things"],
            bad_things=json_data["bad_things"],
        ),
        population=Population(
            current=json_data["population"],
            max=json_data["housing_cap"],
            available_for_troops=json_data["peasants_available_for_troops"],
            rationing=json_data["rationing"],
        ),
        gold=Gold(
            current=json_data["gold"],
            tax_rate=json_data["tax_rate"],
            tax_amount=json_data["tax_amount"],
        ),
        food=Food(
            clock=json_data["food_clock"],
            total=json_data["total_food"],
            months_of_food=json_data["months_of_food"],
            types_eaten=json_data["food_types_eaten"],
            types_available=json_data["food_types_available"],
        ),
        audio=Audio(
            speech=json_data["speechFileName"],
            music=json_data["musicFileName"],
            bink=json_data["binkFileName"],
        ),
        religion=Religion(
            num_priests=json_data["num_priests"],
            blessed_percent=json_data["blessed_percent"],
            blessed_next_level_at=json_data["blessed_next_level_at"],
        ),
        economy=Economy(
            efficiency=json_data["efficiency"],
        ),
        fear_factor=json_data["fear_factor"],
        upcoming_fear_factor=json_data["fear_factor_next_level"],
    )