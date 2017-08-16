# meetup-randomatic

An Azure Function implementation for picking random Meetup attendees, for prize giveaways, heckling, etc.

To use:

1. go to https://www.meetup.com/meetup_api/ to create a Meetup.com API key (if you don't already have one)
1. create a new Azure Function with the code from run.csx in this repository
1. add a "meetupName" (no quotes) configuration value to your function app, with the unique URL suffix of your meetup (http://www.meetup.com/YOUR-UNIQUE-SUFFIX) just the suffix, not the whole URL
1. add a "meetupKey" (no quotes) configuration value to your function app, with your API key
1. you can test the API by POST-ing JSON with the date of the meetup event you want to randomize attendees for, like this:

    { 'date': '5/5/2015' }

1. profit!
