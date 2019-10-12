#!/usr/bin/env python2
import argparse
import datetime
import os
import random
import requests
import shlex
import signal
import ssl
import subprocess
import time
from bs4 import BeautifulSoup
from requests.adapters import HTTPAdapter
#from urllib3.poolmanager import PoolManager
from requests.packages.urllib3.poolmanager import PoolManager

VERBOSE = False
FNULL = open(os.devnull, 'w')


def debug(s):
    if VERBOSE:
        print(s)


class ConnectionAdapter(HTTPAdapter):
    def init_poolmanager(self, connections, maxsize, block=False):
        self.poolmanager = PoolManager(num_pools=connections, maxsize=maxsize,
                                       block=block, ssl_version=ssl.PROTOCOL_TLSv1)


class Connection:
    def __init__(self, username, password):
        LOGIN_URL = 'https://chaturbate.com/auth/login/'
        self.session = requests.session()
        self.session.mount('https://', ConnectionAdapter())
        response = self.session.get(LOGIN_URL)
        if response.status_code >= 400:
            self.close()
            raise Exception('Could not connect to login page')
        self.csrftoken = response.cookies['csrftoken']
        response = self.session.post(LOGIN_URL,
                                     data=dict(username=username, password=password,
                                               csrfmiddlewaretoken=self.csrftoken, next='/'),
                                     headers=dict(Referer=LOGIN_URL))

        if response.status_code >= 400:
            self.close()
            raise Exception('Could not login')
        debug('Logged in!')

    def model_list(self):
        FOLLOWED_URL = 'https://chaturbate.com/followed-cams/'
        response = self.session.get(FOLLOWED_URL)
        if response.status_code >= 400:
            raise Exception('Could not retrieve model list')
        soup = BeautifulSoup(response.text)
        debug('Fetched model list')
        return [n.parent.parent.a.text[1:] for n in soup.findAll('li', class_="cams")
                if n.text != "offline" and n.parent.parent.parent.div.text != "IN PRIVATE"]

    def model_links(self, model_list=None):
        if not model_list:
            model_list = self.model_list()
        timestamp = datetime.datetime.now().strftime('%Y.%m.%d_%H.%M')
        return [{'url': 'http://chaturbate.com/' + n, 'filename': n + '_' + timestamp + '.flv'}
                for n in model_list]

    def close(self):
        self.session.close()


class BCap:
    def __init__(self, username, password,
                 output_folder=os.getcwd() + os.sep + 'captured' + os.sep,
                 livestreamer='livestreamer'):
        self.connection = Connection(username, password)
        self.model_info = []
        self.output_folder = output_folder
        self.livestreamer = livestreamer

    def enqueue(self, model_links=None):
        if not model_links:
            model_links = self.connection.model_links()

        for link in model_links:
            flag = False
            for info in self.model_info:
                if link['url'] == info['url']:
                    flag = True
            if not flag:
                self._spawn(link)

    def kill_zombies(self):
        for info in self.model_info:
            if info['process'].poll() is not None:
                debug('Removing subprocess for ' + info['filename'])
                try:
                    os.kill(info['process'].pid, signal.SIGTERM)
                except OSError:
                    debug('Couldn\'t SIGTERM ' + str(info['process'].pid) + ', is process already dead?')
                self.model_info.remove(info)

    def close(self):
        self.kill_zombies()
        self.connection.close()

    def _spawn(self, link):
        debug('Creating subprocess for ' + link['filename'])
        if not os.path.exists(self.output_folder):
            os.mkdir(self.output_folder)
        link['process'] = subprocess.Popen(shlex.split('%s --output "%s%s" "%s" best' %
                                           (self.livestreamer, self.output_folder, link['filename'], link['url'])),
                                           stdout=FNULL, stderr=FNULL)
        self.model_info += [link]


def main():
    global VERBOSE
    parser = argparse.ArgumentParser()
    parser.add_argument('-v', '--verbose', help='Print debug output', action='store_true')
    parser.add_argument('-o', '--output-directory', help='Output directory', type=str)
    parser.add_argument('-l', '--livestreamer', help='livestreamer executable', type=str)
    parser.add_argument('username', help='Chaturbate username', type=str)
    parser.add_argument('password', help='Chaturbate passsword', type=str)
    args = parser.parse_args()

    if args.verbose:
        VERBOSE = True

    if args.livestreamer:
        livestreamer = args.livestreamer
    else:
        livestreamer = '/usr/local/bin/livestreamer'

    if not os.path.exists(livestreamer):
        print('Could not find livestreamer at: ' + livestreamer)
        exit(1)

    if args.output_directory:
        client = BCap(args.username, args.password, args.output_directory, livestreamer=livestreamer)
    else:
        client = BCap(args.username, args.password, livestreamer=livestreamer)

    def sigchld_handler(signum, stack):
        try:
            debug('SIGCHLD received')
            client.kill_zombies()
        except Exception as e:
            debug('Caught sigchld_handler exception: ' + str(e))

    signal.signal(signal.SIGCHLD, sigchld_handler)

    while True:
        try:
            client.enqueue()
        except Exception as e:
            debug('Caught enqueue exception: ' + str(e))
            client.close()
            client = None
            time.sleep(60)
            while True:
                try:
                    if args.output:
                        client = BCap(args.username, args.password, args.output_directory,
                                      livestreamer=livestreamer)
                    else:
                        client = BCap(args.username, args.password, livestreamer=livestreamer)
                    break
                except Exception as e:
                    client.close()
                    debug('Caught instantiation exception: ' + str(e))
                    time.sleep(random.randrange(180, 300))
        time.sleep(random.randrange(30, 120))

if __name__ == '__main__':
    main()